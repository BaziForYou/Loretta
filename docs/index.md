# Loretta
A C# (G)Lua lexer, parser, code analysis, transformation and code generation toolkit.

![Repo Stats](https://repobeats.axiom.co/api/embed/089a9f7dae190ea8dd0fc0750abbebceea3e86dd.svg "Repobeats analytics image")

This is (another) rewrite from scratch based on Roslyn and [The Complete Syntax of Lua](https://www.lua.org/manual/5.2/manual.html#9) with a few extensions:
1. Operators introduced in Garry's Mod Lua (glua):
    - `&&` for `and`;
    - `||` for `or`;
    - `!=` for `~=`;
    - `!` for `not`;
2. Comment types introduced in Garry's Mod Lua (glua):
    - C style single line comment: `// ...`;
    - C style multi line comment: `/* */`;
3. Characters accepted as part of identifiers by LuaJIT (emojis, non-rendering characters, [or basically any byte above `127`/`0x7F`](https://github.com/LuaJIT/LuaJIT/blob/e9af1abec542e6f9851ff2368e7f196b6382a44c/src/lj_char.c#L10-L13));
4. Luau syntax (partial):
    - Roblox compound assignment: `+=`, `-=`, `*=`, `/=`, `^=`, `%=`, `..=`;
    - If expressions: `if a then b else c` and `if a then b elseif c then d else e`;
5. Lua 5.3 bitwise operators;
6. FiveM's hash string syntax (only parsing, manual node creation currently not possible);
7. Continue support. The following options are available:
    - No continue at all;
    - Roblox's `continue` which is a contextual keyword;
    - Garry's Mod's `continue` which is a full fledged keyword.