using System;
using CGImageFiltering.App.Models.Editor.Interfaces;
using GCImageFiltering.Core.Filters.Interfaces;

namespace CGImageFiltering.App.Models.Editor;

public class FilterOption : IFilterOption
{
    public string Name { get; set; }
    public Func<IFilter> FilterFactory { get; set; }
    
    public FilterOption(string name, Func<IFilter> filterFactory)
    {
        Name = name;
        FilterFactory = filterFactory;
    }
}