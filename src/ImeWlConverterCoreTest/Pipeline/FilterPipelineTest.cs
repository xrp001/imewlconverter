using System.Collections.Generic;
using System.Linq;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Core.Pipeline;
using Xunit;

namespace Studyzy.IMEWLConverter.Tests.Pipeline;

public class FilterPipelineTest
{
    [Fact]
    public void Apply_WithNoFilters_ReturnsAllEntries()
    {
        var pipeline = new FilterPipeline();
        var entries = new List<WordEntry>
        {
            new() { Word = "你好", Rank = 100 },
            new() { Word = "世界", Rank = 50 }
        };

        var result = pipeline.Apply(entries);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Apply_WithFilter_RemovesNonMatchingEntries()
    {
        var filter = new MinLengthFilter(2);
        var pipeline = new FilterPipeline(filters: [filter]);
        var entries = new List<WordEntry>
        {
            new() { Word = "好", Rank = 100 },
            new() { Word = "你好", Rank = 50 },
            new() { Word = "你好世界", Rank = 10 }
        };

        var result = pipeline.Apply(entries);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.Word.Length >= 2));
    }

    [Fact]
    public void Apply_WithBatchFilter_ProcessesAllEntries()
    {
        var batchFilter = new DeduplicateFilter();
        var pipeline = new FilterPipeline(batchFilters: [batchFilter]);
        var entries = new List<WordEntry>
        {
            new() { Word = "你好", Rank = 100 },
            new() { Word = "你好", Rank = 50 },
            new() { Word = "世界", Rank = 10 }
        };

        var result = pipeline.Apply(entries);

        Assert.Equal(2, result.Count);
    }

    private sealed class MinLengthFilter(int minLength) : IWordFilter
    {
        public bool ShouldKeep(WordEntry entry) => entry.Word.Length >= minLength;
    }

    private sealed class DeduplicateFilter : IBatchFilter
    {
        public IReadOnlyList<WordEntry> Filter(IReadOnlyList<WordEntry> entries)
        {
            return entries.DistinctBy(e => e.Word).ToList();
        }
    }
}
