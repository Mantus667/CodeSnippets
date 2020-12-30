# The anatomy of Rule Engine

## Context

Context is a shared state that a collection of rules are running in. Context could be separated out as its own engine agnostic state machine mutable or immutable.

## Rule

A block of code, a computation or logic unit that follows Single Responsibility and offloads side effect to Context if any.
Some think that Single Responsibility means doing one thing. However, the advocate of SOLID Robert C. Martin who started off SOLID (although he did not invent the abbreviation) says that SR means single reason to change. In real production code, I found the latter is more practical because it is literally impossible to make each single method or class do just one thing or we might get buried in what I call the function-inception — layers of layers those little one liner functions although do one thing but really have no point because the function names does not give us more understanding than that one liner code itself! And we’re really just abusing Stack in memory.
Also it makes sense because the whole idea of SOLID is being agile — how to go about responding to change. And if a unit of code has a single reason to change it is highly likely all its sub-units have the same frequency of change. Although it is doing multiple things, because this single reason, all its sub-units are cohesively glued together under the same theme.

## Engine

A generic, business rule agnostic unit of mechanism that runs a collection of rules in a given configuration — async, pipe, first-in-first-run, parallel, exclusive-or, logical-and, etc.
The key is to separate the business rules vs running the rules. So rules and rule-runner can evolve independently.

The key is to separate the business rules vs running the rules. So rules and rule-runner can evolve independently.