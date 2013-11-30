#!/bin/sh

if [ $# -eq 0 ]; then
    MYNAME=`basename $0`
    echo "$MYNAME - Wakes you up at a certain date."
    echo "Usage: $MYNAME <date>"
    exit 1
fi

NOW=`date +%s`
THEN=`date +%s -d "$*"`
[ $? -ne 0 ] && exit 1
DIFF=$(( $THEN - $NOW ))

echo "Now is `date`."

if [ $DIFF -lt 0 ]; then
    echo "Error: `date -d \"$*\"` is in the past."
    exit 1
fi

echo "I’ll wake you up on `date -d \"$*\"`"
echo "(In $(( $DIFF / 3600 )) hours and $(( $DIFF / 60 % 60 )) minutes.)"

sleep $(( $THEN - $NOW ))

while true ; do
    echo "WAKE UP!"
    beep -f 440 -n -f 880 -n -f 1780 -D 2000
done
