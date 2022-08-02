#!/usr/bin/env python3
# coding: utf8

import argparse
import fnmatch
import glob
import os
import subprocess
import sys

parser = argparse.ArgumentParser(description='Executes the given command for each matching file')
parser.add_argument('command', type=str, 
                    help='The command to execute for each matching file')
parser.add_argument('--pattern', type=str, 
                    help='The glob pattern that is used to match the files', required=True)
parser.add_argument('--exclude', type=str, default=os.environ.get('GLOB_EXEC_EXCLUDE'),
                    help='An optional semicolon delimited list of glob patterns used to exclude unwanted files')

args = parser.parse_args()
exclude = None if args.exclude is None else args.exclude.split(';')

def matches(file, patterns):
    if not patterns:
        return False
    for pattern in patterns:
        if fnmatch.fnmatch(file, pattern):
            return True
    return False

for file in glob.iglob(args.pattern, recursive=True):
    if (not args.exclude) or (not matches(file, exclude)):
        print(f'Processing file \'{file}\' ...')
        sys.stdout.flush()

        result = subprocess.run(args.command.replace('{}', f'"{file}"'), shell=True)
        if result.returncode != 0:
            exit(result.returncode)
