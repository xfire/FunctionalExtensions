#!/bin/bash
#
# vim:syntax=sh:sw=4:ts=4:expandtab

SLN="FunctionalExtensions.sln"

echo "Building Solution: $SLN"
xbuild /nologo /verbosity:quiet $SLN
