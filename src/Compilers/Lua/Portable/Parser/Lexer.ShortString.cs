﻿using System.Diagnostics;
using System.Text;
using GParse.Utilities;
using Loretta.CodeAnalysis.Text;
using Loretta.Utilities;

namespace Loretta.CodeAnalysis.Lua.Syntax
{
    internal sealed partial class Lexer
    {
        private string ParseShortString()
        {
            var strStart = this._reader.Position;
            var parsed = new StringBuilder();
            var delim = this._reader.Read()!.Value;
            RoslynDebug.Assert(delim is '"' or '\'');

            while (this._reader.Peek() is char peek && peek != delim)
            {
                var charStart = this._reader.Position;
                switch (peek)
                {
                    #region Escapes

                    case '\\':
                    {
                        var escapeStart = this._reader.Position;
                        this._reader.Advance(1);

                        switch (this._reader.Peek())
                        {
                            case '\r':
                                if (this._reader.IsAt('\n', 1))
                                {
                                    this._reader.Advance(2);
                                    parsed.Append("\r\n");
                                }
                                else
                                {
                                    this._reader.Advance(1);
                                    parsed.Append('\r');
                                }
                                break;

                            case 'a':
                                this._reader.Advance(1);
                                parsed.Append('\a');
                                break;

                            case 'b':
                                this._reader.Advance(1);
                                parsed.Append('\b');
                                break;

                            case 'f':
                                this._reader.Advance(1);
                                parsed.Append('\f');
                                break;

                            case 'n':
                                this._reader.Advance(1);
                                parsed.Append('\n');
                                break;

                            case 'r':
                                this._reader.Advance(1);
                                parsed.Append('\r');
                                break;

                            case 't':
                                this._reader.Advance(1);
                                parsed.Append('\t');
                                break;

                            case 'v':
                                this._reader.Advance(1);
                                parsed.Append('\v');
                                break;

                            case '\\':
                                this._reader.Advance(1);
                                parsed.Append('\\');
                                break;

                            case '\n':
                                this._reader.Advance(1);
                                parsed.Append('\n');
                                break;

                            case '\'':
                                this._reader.Advance(1);
                                parsed.Append('\'');
                                break;

                            case '"':
                                this._reader.Advance(1);
                                parsed.Append('"');
                                break;

                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            {
                                var parsedCharInteger = parseDecimalInteger(escapeStart);
                                if (parsedCharInteger != char.MaxValue)
                                    parsed.Append(parsedCharInteger);
                                break;
                            }

                            case 'x':
                            {
                                this._reader.Advance(1);
                                var parsedCharInteger = parseHexadecimalInteger(escapeStart);
                                if (parsedCharInteger != char.MaxValue)
                                    parsed.Append(parsedCharInteger);

                                if (!this._luaOptions.AcceptHexEscapesInStrings)
                                {
                                    var span = TextSpan.FromBounds(escapeStart, this._reader.Position);
                                    var location = new TextLocation(this._text, span);
                                    this.Diagnostics.ReportHexStringEscapesNotSupportedInVersion(location);
                                }
                            }
                            break;

                            default:
                            {
                                // Skip the character after the escape.
                                this._reader.Advance(1);
                                var span = TextSpan.FromBounds(escapeStart, this._reader.Position);
                                var location = new TextLocation(this._text, span);
                                this.Diagnostics.ReportInvalidStringEscape(location);
                            }
                            break;
                        }
                    }
                    break;

                    #endregion Escapes

                    case '\r':
                    {
                        if (this._reader.IsAt('\n', 1))
                        {
                            this._reader.Advance(2);
                            parsed.Append("\r\n");
                        }
                        else
                        {
                            this._reader.Advance(1);
                            parsed.Append('\r');
                        }

                        var span = TextSpan.FromBounds(charStart, this._reader.Position);
                        var location = new TextLocation(this._text, span);
                        this.Diagnostics.ReportUnescapedLineBreakInString(location);
                    }
                    break;

                    case '\n':
                    {
                        this._reader.Advance(1);
                        parsed.Append('\n');

                        var span = TextSpan.FromBounds(charStart, this._reader.Position);
                        var location = new TextLocation(this._text, span);
                        this.Diagnostics.ReportUnescapedLineBreakInString(location);
                    }
                    break;

                    default:
                        this._reader.Advance(1);
                        parsed.Append(peek);
                        break;
                }
            }

            if (this._reader.IsNext(delim))
            {
                this._reader.Advance(1);
            }
            else
            {
                var span = TextSpan.FromBounds(strStart, this._reader.Position);
                var location = new TextLocation(this._text, span);
                this.Diagnostics.ReportUnfinishedString(location);
            }

            return parsed.ToString();

            char parseDecimalInteger(int start)
            {
                var readChars = 0;
                var num = 0;
                char ch;
                while (readChars < 3 && CharUtils.IsDecimal(ch = this._reader.Peek().GetValueOrDefault()))
                {
                    this._reader.Advance(1);
                    num = (num * 10) + (ch - '0');
                    readChars++;
                }

                if (readChars < 1 || num > 255)
                {
                    var span = TextSpan.FromBounds(start, this._reader.Position);
                    var location = new TextLocation(this._text, span);
                    this.Diagnostics.ReportInvalidStringEscape(location);
                    return char.MaxValue;
                }

                return (char) num;
            }

            char parseHexadecimalInteger(int start)
            {
                var readChars = 0;
                var num = (byte) 0;
                while (readChars < 2)
                {
                    var peek = this._reader.Peek().GetValueOrDefault();
                    if (CharUtils.IsDecimal(peek))
                    {
                        this._reader.Advance(1);
                        num = (byte) ((num << 4) | (peek - '0'));
                    }
                    else if (CharUtils.IsHexadecimal(peek))
                    {
                        this._reader.Advance(1);
                        num = (byte) ((num << 4) | (10 + CharUtils.AsciiLowerCase(peek) - 'a'));
                    }
                    else
                    {
                        break;
                    }
                    readChars++;
                }

                if (readChars < 1)
                {
                    var span = TextSpan.FromBounds(start, this._reader.Position);
                    var location = new TextLocation(this._text, span);
                    this.Diagnostics.ReportInvalidStringEscape(location);
                    return char.MaxValue;
                }

                return (char) num;
            }
        }
    }
}