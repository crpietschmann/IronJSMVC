// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

using System;
using System.Text;
using System.Text.RegularExpressions;

// Credited to: http://haacked.com/archive/2009/04/22/scripted-db-views.aspx

namespace IronJSMVC.ViewEngine
{
    public class JavaScriptTemplate
    {
        public JavaScriptTemplate(string templateContents)
        {
            this.Template = templateContents;
        }

        public string Template { get; private set; }

        public string ToScript()
        {
            return this.ToScript(null);
        }

        public string ToScript(string methodName)
        {
            var sb = new StringBuilder();
            this.RenderScript(methodName, sb);
            return sb.ToString();
        }

        public void RenderScript(string methodName, StringBuilder builder)
        {
            var contents = this.Template;

            if (!String.IsNullOrEmpty(methodName))
            {
                builder.AppendLine("function " + methodName + "(){");
            }

            var scriptBlocks = new Regex("<%.*?%>", RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = scriptBlocks.Matches(contents);

            var currentIndex = 0;
            var blockBeginIndex = 0;

            foreach (Match match in matches)
            {
                blockBeginIndex = match.Index;
                var block = JavaScriptBlock.Parse(contents.Substring(currentIndex, blockBeginIndex - currentIndex));

                if (!String.IsNullOrEmpty(block.Contents))
                {
                    builder.AppendLine(block.Contents);
                }

                block = JavaScriptBlock.Parse(match.Value);
                builder.AppendLine(block.Contents);
                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < contents.Length - 1)
            {
                var endBlock = JavaScriptBlock.Parse(contents.Substring(currentIndex));
                builder.AppendLine(endBlock.Contents);
            }

            if (!String.IsNullOrEmpty(methodName))
            {
                builder.AppendLine();
                builder.AppendLine("}");
            }

        }
    }
}
