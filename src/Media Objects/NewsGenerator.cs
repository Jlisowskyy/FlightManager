using proj.Interfaces;

namespace proj.Media_Objects;

public class NewsGenerator(List<IReportable> reportables, List<Reporter> reporters)
{
// ------------------------------
// Class interaction
// ------------------------------

    public string? GenerateNextNews()
    {
        if (_reportables.Count == 0) return null;
        
        if (_reportableInd >= _reportables.Count)
        {
            _reportableInd = 0;
            _reporterInd++;
        }

        if (_reporterInd >= _reporters.Count)
            return null;

        return _reportables[_reportableInd++].AcceptReporter(_reporters[_reporterInd]);
    }
    
// ------------------------------
// Class fields
// ------------------------------

    private int _reporterInd = 0;
    private int _reportableInd = 0;
    
    private List<IReportable> _reportables = reportables;
    private List<Reporter> _reporters = reporters;
}