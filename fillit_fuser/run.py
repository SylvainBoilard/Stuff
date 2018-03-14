#!/usr/bin/env python3

import os
import subprocess
from random import seed
from random import randrange as rand

seed()

def get_rand_dir():
    return [
        (0, -1),
        (0, 1),
        (-1, 0),
        (1, 0)
    ][rand(4)]

def gen_valid():
    p = [['.' for _ in range(4)] for _ in range(4)]
    x = rand(1, 3)
    y = rand(1, 3)
    c = 0
    while c < 4:
        (dx, dy) = get_rand_dir()
        x = max(0, min(3, x + dx))
        y = max(0, min(3, y + dy))
        if (p[x][y] == '.'):
            p[x][y] = '#'
            c += 1
    return p

def put_piece(piece, fd):
    for row in piece:
        os.write(fd, "".join(row).encode())
        os.write(fd, b"\n")

def gen_test(filename):
    count = rand(5, 12)
    fd = os.open(filename, os.O_WRONLY | os.O_CREAT | os.O_TRUNC | os.O_APPEND)
    for i in range(count):
        put_piece(gen_valid(), fd)
        if i != count - 1:
            os.write(fd, b"\n")
    os.close(fd)

for t in range(100):
    gen_test("test.txt")
    expected = subprocess.run(["./fillit_ref", "test.txt"], stdout=subprocess.PIPE).stdout
    result = subprocess.run(["./fillit", "test.txt"], stdout=subprocess.PIPE).stdout
    if (expected != result):
        print("Test {} failed.".format(t))
        os.rename("test.txt", "test_{}.txt".format(t))
try:
    os.remove("test.txt")
except FileNotFoundError:
    pass
