// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

using System;

// Credited to: http://haacked.com/archive/2009/04/22/scripted-db-views.aspx

namespace IronJSMVC.ViewEngine
{
    public class JavaScriptBlock
    {
        public static JavaScriptBlock Parse(string block)
        {
            return new JavaScriptBlock(block);
        }

        public string Contents { get; private set; }

        public JavaScriptBlock(string block)
        {
            bool ignoreNewLine = ignoreNextNewLine;

            if (String.IsNullOrEmpty(block))
            {
                this.Contents = string.Empty;
                return;
            }

            int endOffset = 4;
            if (block.EndsWith("-%>"))
            {
                endOffset = 5;
                ignoreNextNewLine = true;
            }
            else
            {
                ignoreNextNewLine = false;
            }

            if (block.StartsWith("<%=") || block.StartsWith("<%:"))
            {
                int outputLength = block.Length - endOffset - 1;
                if (outputLength < 1)
                    throw new InvalidOperationException("Started a '<%=' block without ending it.");

                string output = block.Substring(3, outputLength).Trim();
                if (block.StartsWith("<%="))
                {
                    Contents = String.Format("response.Write({0});", output).Trim();
                }
                else
                {
                    Contents = String.Format("response.Write(server.htmlEncode({0}));", output).Trim();
                }
                return;
            }

            if (block.StartsWith("<%"))
            {
                Contents = block.Substring(2, block.Length - endOffset).Trim();
                return;
            }

            if (ignoreNewLine)
            {
                block = block.Trim();
            }

            block = block.Replace(@"\", @"\\");
            block = block.Replace(Environment.NewLine, "\\r\\n");
            block = block.Replace(@"""", @"\""");

            if (block.Length > 0)
            {
                Contents = string.Format("response.Write(\"{0}\");", block);
            }
        }

        [ThreadStatic]
        static bool ignoreNextNewLine = false;
    }
}
