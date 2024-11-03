﻿using System.ComponentModel.DataAnnotations;

namespace Medical.Client.Features.SnippetLibrary;

public class CodeSnippet
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string Content { get; set; }
    public string Language { get; set; }
    public string SourceUrl { get; set; }
}
