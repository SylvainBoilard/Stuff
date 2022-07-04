#!/bin/sh

# This script only tunnels IPv4 traffic, but can do so through an IPv6 tunnel.
# The ssh server configuration of the remote side needs to allow root login and
# tunneling.

# Replace with remote host IP address. Can be an IPv6 address, in which case
# you do not need to setup the route to the remote on the local endpoint.
REMOTE_HOST=no_remote_host
REMOTE_USER=root
REMOTE_PORT=22
IDENTITY_FILE=$HOME/.ssh/id_rsa

LOCAL_GATEWAY=$(ip route show default | cut -d ' ' -f 3)

REMOTE_INTERFACE=eth0

TUN_LOCAL=0
TUN_REMOTE=0
TUN_IP_MASK=24
TUN_IP_LOCAL=192.168.234.100
TUN_IP_REMOTE=192.168.234.1

ssh ${REMOTE_USER}@${REMOTE_HOST} -p ${REMOTE_PORT} -i ${IDENTITY_FILE} "\
	echo '[R] Setting remote endpoint up (1/2)...' \
	&& echo '[R] Inserting kernel module.' \
	&& modprobe tun \
	&& echo '[R] Enabling IP forwarding.' \
	&& echo 1 > /proc/sys/net/ipv4/ip_forward \
	&& echo '[R] Done.'"

echo '[L] Setting local endpoint up (1/2)...'
echo '[L] Inserting kernel module.'
modprobe tun
echo '[L] Done.'

ssh -w ${TUN_LOCAL}:${TUN_REMOTE} -f ${REMOTE_USER}@${REMOTE_HOST} -p ${REMOTE_PORT} -i ${IDENTITY_FILE} "\
	echo '[R] Setting remote endpoint up (2/2)...' \
	&& echo '[R] Adding address to tun device.' \
	&& ip addr add ${TUN_IP_REMOTE}/${TUN_IP_MASK} dev tun${TUN_REMOTE} \
	&& echo '[R] Bringing tun device up.' \
	&& ip link set tun${TUN_REMOTE} up \
	&& echo '[R] Configuring netfilter.' \
	&& { iptables -t nat -C POSTROUTING -o ${REMOTE_INTERFACE} -j MASQUERADE > /dev/null 2>&1 \
	  || iptables -t nat -A POSTROUTING -o ${REMOTE_INTERFACE} -j MASQUERADE ; } \
	&& echo '[R] Done.'"

sleep 3

echo '[L] Setting local endpoint up (2/2)...'
echo '[L] Adding address to tun device.'
ip addr add ${TUN_IP_LOCAL}/${TUN_IP_MASK} dev tun${TUN_LOCAL}
echo '[L] Bringing tun device up.'
ip link set tun${TUN_LOCAL} up
echo '[L] Configuring routes.'
ip route add ${REMOTE_HOST} via ${LOCAL_GATEWAY} # Comment out if remote is an IPv6 address.
ip route add 0.0.0.0/1 via ${TUN_IP_REMOTE}
ip route add 128.0.0.0/1 via ${TUN_IP_REMOTE}
echo '[L] Done.'
