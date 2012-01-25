#!/bin/bash
#
# vim:syntax=sh:sw=4:ts=4:expandtab

SLN="FunctionalExtensions.sln"

Tools/react.py . -p '*.cs' "./build.sh && nunit-console $SLN -nologo"
