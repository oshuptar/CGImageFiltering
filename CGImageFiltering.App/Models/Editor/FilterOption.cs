using System;
using CGImageFiltering.App.Models.Editor.Interfaces;
using GCImageFiltering.Core.Filters.Interfaces;

namespace CGImageFiltering.App.Models.Editor;

public class FilterOption : IFilterOption
{
    public string Name { get; set; }
    public string? ParameterName { get; }
    public double? MinParameterValue { get; }
    public double? MaxParameterValue { get; }
    public double? ParameterIncrement { get; }
    public Func<double?, IFilter> FilterFactory { get; set; }

    public double? Parameter { get; set; }
    public FilterOption(string name, double parameter, string parameterName,double min, double max, double increment ,Func<double?, IFilter> filterFactory)
    {
        Name = name;
        FilterFactory = filterFactory;
        Parameter = parameter;
        ParameterName = parameterName;
        MinParameterValue = min;
        MaxParameterValue = max;
        ParameterIncrement = increment;
    }
    
    public FilterOption(string name, Func<double?, IFilter> filterFactory)
    {
        Name = name;
        FilterFactory = filterFactory;
    }
}