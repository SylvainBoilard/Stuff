/*
  Typoglycemier, a typoglicemic scribe.

  Copyright (C) 2013 Sylvain Boilard <boilard@crans.org>
  All rights reserved.

  Distributed under the terms of the simplified BSD license.

  This program writes to stdout the text entered on stdin, taking each word and
  shuffling their characters except for the first and last character of each
  word. Because of typoglicemia, the text is still readable with a bit of
  effort. This effect can be used to dissimulate a piece of text from a robot
  and make for a basic captcha, or make sure a text can only be read if one
  wants to and help dissimulate spoilers about films or video games where
  discussion forums does not provide more efficient dissimulation methods.

  A word is considered to be a suite of letters from A to Z regardless of case.
  Incidentally, accented characters and most Unicode characters that one could
  use to make a legit word will not count as word components, and will instead
  act as word separators.
*/

#include <stdio.h>
#include <stdlib.h>
#include <time.h>

#define BASE_SIZE 64

int main(void)
{
    char* buffer = malloc(sizeof(char) * BASE_SIZE);
    unsigned int buffer_size = BASE_SIZE;
    unsigned int buffer_used = 0;
    char char_buffer;

    srand(time(NULL));

    while ((char_buffer = getchar()) != EOF)
    {
        const char char_buffer_norm = char_buffer | 0x20;
        if (char_buffer_norm >= 'a' && char_buffer_norm <= 'z')
        {
            if (buffer_used == buffer_size)
            {
                buffer_size *= 2;
                buffer = realloc(buffer, sizeof(char) * buffer_size);
            }
            buffer[buffer_used++] = char_buffer;            
        }
        else
        {
            if (buffer_used > 3)
            {
                unsigned int i;
                /* This is a Knuth's shuffle, modified so that we make sure
                   each character is moved away from its original position. */
                for (i = buffer_used - 3; i; --i)
                {
                    unsigned int index = (rand() % i) + 1;
                    const char temp = buffer[index];
                    buffer[index] = buffer[i + 1];
                    buffer[i + 1] = temp;
                }
            }
            if (buffer_used == buffer_size)
            {
                buffer_size *= 2;
                buffer = realloc(buffer, sizeof(char) * buffer_size);
            }
            buffer[buffer_used] = '\0';
            printf(buffer);
            putchar(char_buffer);
            buffer_used = 0;
        }
    }

    free(buffer);

    return 0;
}
