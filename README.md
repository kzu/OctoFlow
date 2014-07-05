GitFlow Process
==============

Process flow status generator based on GitFlow and using GitHub APIs

The goal is to be based on heuristics and not require much label management in GitHub, which adds unnecessary overhead.

Current heuristics applied:

- 'Issue Type' labels are those that have only letters and optionally whitespace (for multi-word). Examples:
  - Labels considered 'issue type': 'Story', 'User Story', 'Bug'
  - Labels not considered 'issue type': '#Doco', '1 - Doing', '+QA', '✓Doc'

- 'Release' labels are those that are a version number (MAJOR.MINOR with optional .PATCH), with optional "v" or "V" prefix. Typically, they match a corresponding repository Tag that makes a proper [Release in GitHub](https://help.github.com/articles/about-releases). Examples: 'v1.0', '3.2.3'.

All three flows, Dev, QA and Doc have only three states: ToDo, Doing, Done.

> Note: we use the [checkmark symbol](http://www.fileformat.info/info/unicode/char/2713/browsertest.htm) to denote done items.


## Dev Flow

Dev items are grouped by milestone. No labels are required for the flow.

- Backlog items are those that:
  - Are open
  - Have no milestone assigned

Flow states:
- ToDo: open items that are not assigned
- Doing: open items that are assigned
- Done: closed items

## QA Flow

QA is a required activity with optional opt-out. Items are grouped by release label. Labels: 
- -QA: explicit opt-out of QA.
- +QA: optional label, since everything without -QA needs QA.
- ✓QA: the issue has been verified by QA.

QA items are those without -QA label and closed. +QA label is optional.

Flow states:
- ToDo: unassigned QA items without the ✓QA label
- Doing: assigned QA items without the ✓QA label
- Done: QA items with the ✓QA label


## Doc Flow

Doc is an opt-in activity. Items are also grouped by release label. Labels: 

- +Doc: the issue needs user documentation.
- ✓Doc: documentation has been completed. Developers could use this to verify the docs.


Flow states:
- ToDo: unassigned closed items with +Doc label 
- Doing: assigned closed items with +Doc label
- Done: closed items with the ✓Doc label
