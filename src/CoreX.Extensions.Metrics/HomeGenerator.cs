using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Metrics
{
    public class HomeGenerator
    {
        public static string BasicHtml()
        {
            var builder = new StringBuilder();
            builder.Append($"<section>");
            builder.Append($"<div class='model-container' style='float:right; font-size:12px;line-height:1.4em;margin-top:-80px'><span class='model-box'>");
            builder.Append($"<label>Name:</label> {System.Reflection.Assembly.GetEntryAssembly().GetName().Name}<br>");
            builder.Append($"<label>Ver:</label> {System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()}<br>");
            builder.Append($"<label>Server:</label> {Environment.MachineName}<br>");
            builder.Append($"<label>OS:</label> {Environment.OSVersion}<br>");
            builder.Append($"<label>CPUs:</label> {Environment.ProcessorCount.ToString()}<br>");
            builder.Append($"<label>Memory:</label> {Environment.WorkingSet/1024/1024} MB<br>");
            builder.Append($"</span></div></section>");

            return builder.ToString();
        }
    }
}
