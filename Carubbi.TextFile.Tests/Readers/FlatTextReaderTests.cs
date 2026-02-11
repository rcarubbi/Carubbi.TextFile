using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.Configuration;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Readers;

public class FlatTextReaderTests
{
    [Fact]
    public async Task GivenValidDelimitedFileWithNullValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          #,Name,DOB,Children
                          001,Raphael Carubbi Neto,29/09/1981,2
                          002,,,1
                          003,Bob B.,30/05/2002
                          """;

        File.WriteAllText("test1.txt", fileContent);

        var reader = new FlatTextFileReader<RecordExample>(new ReadingOptions { SkipHeader = true, Mode = ContentMode.Delimited });

        // act
        var result = await reader.ReadFile("test1.txt");

        // assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Raphael Carubbi Neto");
        result[0].DateOfBirth.Should().Be(new DateTime(1981, 9, 29));
        result[0].Children.Should().Be(2);

        result[1].Name.Should().BeNull();
        result[1].DateOfBirth.Should().BeNull();
        result[1].Children.Should().Be(1);

        result[2].Name.Should().Be("Bob B.");
        result[2].DateOfBirth.Should().Be(new DateTime(2002, 5, 30));
        result[2].Children.Should().BeNull();
    }

    [Fact]
    public async Task GivenValidPositionalFileWithEmptyValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          ###Name (20) DOB (10) Children (2)
                          001Raphael Carubbi Neto29/09/198102
                          002John Doe                      01
                          003Bob B.              30/05/2002  
                          """;

        File.WriteAllText("test2.txt", fileContent);

        var reader = new FlatTextFileReader<RecordExample>(new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

        // act
        var result = await reader.ReadFile("test2.txt");

        // assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Raphael Carubbi Neto");
        result[0].DateOfBirth.Should().Be(new DateTime(1981, 9, 29));
        result[0].Children.Should().Be(2);

        result[1].Name.Should().Be("John Doe");
        result[1].DateOfBirth.Should().BeNull();
        result[1].Children.Should().Be(1);

        result[2].Name.Should().Be("Bob B.");
        result[2].DateOfBirth.Should().Be(new DateTime(2002, 5, 30));
        result[2].Children.Should().BeNull();
    }



    [Fact]
    public async Task GivenValidPositionalFileWithEmptyValuesInOptionalFields_WhenReadInParallel_ShouldParse()
    {
        // arrange
        var fileContent = """
                          ###Name (20)         DOB (10)     Children (2)
                          001Raphael Carubbi Neto29/09/198102
                          002John Doe            01/01/198001
                          003Bob B.              30/05/200202
                          004Alice Wonderland    15/07/199301
                          005Michael Smith       20/11/197802
                          006Jessica Johnson     05/03/200000
                          007Emily Davis         11/09/198401
                          008David Brown         22/12/197602
                          009Sarah Wilson        03/06/199301
                          010James Taylor        10/08/198501
                          011Linda Anderson      19/02/197802
                          012Robert Thomas       04/04/199301
                          013Patricia Jackson    28/12/198101
                          014Mary White          31/01/200203
                          015William Harris      02/05/197902
                          016Barbara Martin      18/03/198401
                          017Charles Thompson    09/11/199302
                          018Susan Garcia        23/07/198501
                          019Joseph Martinez     12/12/197801
                          020Margaret Robinson   17/04/199301
                          021Thomas Clark        25/06/198201
                          022Dorothy Rodriguez   30/09/200202
                          023Christopher Lewis   07/10/197902
                          024Karen Lee           16/08/199301
                          025Daniel Walker       27/03/198401
                          026Nancy Hall          13/11/197602
                          027Paul Allen          01/07/198202
                          028Lisa Young          21/02/199301
                          029Mark King           08/05/198501
                          030Betty Wright        06/12/197902
                          031Steven Scott        29/04/199301
                          032Helen Green         24/06/198101
                          033George Adams        05/01/197802
                          034Sandra Baker        15/03/198301
                          035Kenneth Gonzalez    09/07/197901
                          036Donna Nelson        28/02/200203
                          037Edward Carter       03/11/198501
                          038Karen Mitchell      14/06/197902
                          039Brian Perez         19/08/199301
                          040Maria Roberts       11/10/198401
                          041Donald Turner       22/01/197802
                          042Sandra Phillips     04/04/198301
                          043Jason Campbell      23/12/199201
                          044Michelle Parker     17/02/198401
                          045Ronald Evans        10/09/197801
                          046Laura Edwards       06/05/199301
                          047Anthony Collins     08/06/198501
                          048Sarah Stewart       12/12/197802
                          049Kevin Sanchez       01/04/199302
                          050Armando Miani       02/04/199302
                          051Nancy Morris        21/03/198301
                          052Matthew Rogers      13/07/197901
                          053Jessica Reed        27/11/200203
                          054Richard Cook        30/01/198401
                          055Angela Morgan       18/10/197802
                          056Charles Bell        25/06/199301
                          057Stephanie Murphy    05/08/198501
                          058Mark Bailey         09/02/197801
                          059Elizabeth Rivera    26/03/198401
                          060Steven Cooper       16/05/199301
                          061Karen Howard        13/12/197802
                          062Eric Ward           07/07/198301
                          063Sarah Cox           20/01/197902
                          064Michael Diaz        29/10/198501
                          065Kimberly Richardson 17/03/198401
                          066Paul Watson         28/05/199302
                          067Emily Brooks        12/06/197802
                          068Peter Bennett       24/08/198501
                          069Laura Watson        04/03/197901
                          070Kevin Barnes        30/04/200203
                          071Brian Hayes         18/10/198301
                          072Karen James         22/12/197902
                          073Eric Watson         16/05/199201
                          074Lisa Wood           09/11/198501
                          075Ryan Long           21/03/198301
                          076Julie Martinez      11/07/197802
                          077James Powell        07/02/199301
                          078Jessica Russell     14/09/198401
                          079Kevin Griffin       30/01/197901
                          080Flavio Taguada      29/01/197901
                          081Sandra Diaz         22/03/198301
                          082Patrick Torres      03/11/197802
                          083Lisa Ortiz          28/12/199301
                          084George Foster       19/02/198501
                          085Linda Jenkins       10/05/197802
                          086Mary Simmons        15/08/198401
                          087Robert Peterson     06/03/197901
                          088Karen Cox           30/06/199301
                          089Nancy Bailey        14/10/198301
                          090David Bell          02/07/197902
                          091Emily Reed          20/11/198501
                          092Kenneth Sanders     03/01/197901
                          093Lisa Martinez       11/05/198301
                          094Paul Evans          18/07/199301
                          095Elizabeth Hayes     28/09/198401
                          096Chris Mitchell      23/02/197802
                          097Nancy Stewart       06/06/199302
                          098George Watson       15/12/198301
                          099Charles Bryant      01/03/198501
                          100Jennifer Gray       10/08/197901
                          101Mark Alexander      22/04/198301
                          102Kimberly Rodriguez  27/06/199301
                          103John Ross           30/01/198502
                          104Lisa Diaz           08/09/197901
                          105Richard Henderson   13/03/198301
                          106David Moore         26/05/197802
                          107Jessica Cooper      18/10/199301
                          108John Adams          21/02/198501
                          109Jennifer Mitchell   03/11/197801
                          110Thomas Lewis        28/06/199301
                          111Nancy Scott         12/12/198301
                          112Robert Davis        30/04/197901
                          113Jessica Miller      05/01/198501
                          114Kevin Perez         19/03/197902
                          115Elizabeth Taylor    24/08/198301
                          116John Lee            11/11/199301
                          117Karen Thomas        01/07/198501
                          118Donald Robinson     16/02/197901
                          119Sandra Walker       29/05/198301
                          120Brian Anderson      12/03/197802
                          121Nancy Young         22/09/198301
                          122Sarah Mitchell      17/04/198501
                          123Steven Hall         30/11/197901
                          124Mary Scott          04/05/198301
                          125Charles Allen       14/07/199301
                          126Sandra Garcia       27/02/198501
                          127Joseph Turner       01/10/197901
                          128Emily Young         18/03/198301
                          129Michael Campbell    22/08/199301
                          130Sarah Adams         07/01/198501
                          131Daniel Rodriguez    30/05/197901
                          132Nancy Brown         15/09/198301
                          133Kevin Walker        11/12/199301
                          134Karen White         23/03/198501
                          135James Allen         06/11/197902
                          136Jennifer Lewis      17/06/198301
                          137David Nelson        25/02/197801
                          138Sarah Brown         29/10/199302
                          139Paul Scott          14/04/198501
                          140Nancy Martin        19/01/197901
                          141George Johnson      27/07/198301
                          142Linda Adams         01/09/199302
                          143David Lee           15/05/198501
                          144Emily White         24/02/197902
                          145Steven Anderson     30/08/198301
                          146John Martinez       07/03/199301
                          147Kimberly Robinson   20/12/198501
                          148Brian Young         12/02/197901
                          149Lisa Thomas         05/11/198301
                          150Lucas Rivetti       06/11/198301
                          151Michael Clark       19/06/199301
                          152Jennifer Hall       13/01/198501
                          153Karen King          28/03/197901
                          154George Moore        22/08/198301
                          155Nancy Hill          11/10/199302
                          156Jessica Adams       01/12/198501
                          157Paul Nelson         16/02/197902
                          158Sarah Young         28/07/198301
                          159David Thompson      13/09/199301
                          160Emily Martinez      20/04/198501
                          161Steven Miller       05/06/197902
                          162John Anderson       30/11/198301
                          163Nancy Lewis         17/07/199301
                          164Kevin Brown         23/01/198501
                          165Mary Johnson        08/03/197901
                          166Lisa Williams       29/05/198301
                          167David Martin        13/12/197902
                          168Jessica Brown       27/04/199301
                          169Brian Rodriguez     01/06/198501
                          170Sarah Walker        12/01/197902
                          171Emily Davis         26/08/198301
                          172Steven Young        15/11/199302
                          173Jennifer King       06/04/198501
                          174Michael Allen       18/02/197902
                          175Sandra Scott        24/05/198301
                          176George Brown        30/09/199301
                          177Karen Harris        03/07/198501
                          178David Martinez      14/01/197901
                          179Jessica Wilson      27/03/198301
                          180Brian Gonzalez      09/06/199301
                          181Sarah Rodriguez     21/08/198501
                          182Emily Clark         10/02/197902
                          183John Scott          22/10/198301
                          """;

        File.WriteAllText("test7.txt", fileContent);

        var reader = new FlatTextFileReader<RecordExample>(new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

        // act
        var result = await reader.ReadFileInParallel("test7.txt");

        // assert
        result.Should().HaveCount(183);

        var orderedList = result.OrderBy(x => x.Number).ToList();

        var g = result.GroupBy(x => x.Number).Where(x => x.Count() > 1).ToList();

        for (int i = 1; i < orderedList.Count; i++)
        {
            if (orderedList[i].Number != orderedList[i - 1].Number + 1)
            {
                throw new InvalidOperationException($"Item at index {i} with number {orderedList[i].Number} is not sequential after item with number {orderedList[i - 1].Number}");
            }
        }
 
    }


    [Fact]
    public async Task GivenValidPositionalFileWithEmptyValuesInOptionalFields_WhenReadInParallel_ShouldParse2()
    {
        // arrange
        var fileContent = """
                          ###Name (20)         DOB (10)     Children (2)
                          001Raphael Carubbi Neto29/09/198102
                          002John Doe            01/01/198001
                          003Bob B.              30/05/200202
                          004Alice Wonderland    15/07/199301
                          005Michael Smith       20/11/197802
                          006Jessica Johnson     05/03/200000
                          007Emily Davis         11/09/198401
                          008David Brown         22/12/197602
                          009Sarah Wilson        03/06/199301
                          010James Taylor        10/08/198501
                          011Linda Anderson      19/02/197802
                          012Robert Thomas       04/04/199301
                          013Patricia Jackson    28/12/198101
                          014Mary White          31/01/200203
                          015William Harris      02/05/197902
                          016Barbara Martin      18/03/198401
                          017Charles Thompson    09/11/199302
                          018Susan Garcia        23/07/198501
                          019Joseph Martinez     12/12/197801
                          020Margaret Robinson   17/04/199301
                          021Thomas Clark        25/06/198201
                          022Dorothy Rodriguez   30/09/200202
                          023Christopher Lewis   07/10/197902
                          024Karen Lee           16/08/199301
                          025Daniel Walker       27/03/198401
                          026Nancy Hall          13/11/197602
                          027Paul Allen          01/07/198202
                          028Lisa Young          21/02/199301
                          029Mark King           08/05/198501
                          030Betty Wright        06/12/197902
                          031Steven Scott        29/04/199301
                          032Helen Green         24/06/198101
                          033George Adams        05/01/197802
                          034Sandra Baker        15/03/198301
                          035Kenneth Gonzalez    09/07/197901
                          036Donna Nelson        28/02/200203
                          037Edward Carter       03/11/198501
                          038Karen Mitchell      14/06/197902
                          039Brian Perez         19/08/199301
                          040Maria Roberts       11/10/198401
                          041Donald Turner       22/01/197802
                          042Sandra Phillips     04/04/198301
                          043Jason Campbell      23/12/199201
                          044Michelle Parker     17/02/198401
                          045Ronald Evans        10/09/197801
                          046Laura Edwards       06/05/199301
                          047Anthony Collins     08/06/198501
                          048Sarah Stewart       12/12/197802
                          049Kevin Sanchez       01/04/199302
                          050Armando Miani       02/04/199302
                          051Nancy Morris        21/03/198301
                          052Matthew Rogers      13/07/197901
                          053Jessica Reed        27/11/200203
                          054Richard Cook        30/01/198401
                          055Angela Morgan       18/10/197802
                          056Charles Bell        25/06/199301
                          057Stephanie Murphy    05/08/198501
                          058Mark Bailey         09/02/197801
                          059Elizabeth Rivera    26/03/198401
                          060Steven Cooper       16/05/199301
                          061Karen Howard        13/12/197802
                          062Eric Ward           07/07/198301
                          063Sarah Cox           20/01/197902
                          064Michael Diaz        29/10/198501
                          065Kimberly Richardson 17/03/198401
                          066Paul Watson         28/05/199302
                          067Emily Brooks        12/06/197802
                          068Peter Bennett       24/08/198501
                          069Laura Watson        04/03/197901
                          070Kevin Barnes        30/04/200203
                          071Brian Hayes         18/10/198301
                          072Karen James         22/12/197902
                          073Eric Watson         16/05/199201
                          074Lisa Wood           09/11/198501
                          075Ryan Long           21/03/198301
                          076Julie Martinez      11/07/197802
                          077James Powell        07/02/199301
                          078Jessica Russell     14/09/198401
                          079Kevin Griffin       30/01/197901
                          080Flavio Taguada      29/01/197901
                          081Sandra Diaz         22/03/198301
                          082Patrick Torres      03/11/197802
                          083Lisa Ortiz          28/12/199301
                          084George Foster       19/02/198501
                          085Linda Jenkins       10/05/197802
                          086Mary Simmons        15/08/198401
                          087Robert Peterson     06/03/197901
                          088Karen Cox           30/06/199301
                          089Nancy Bailey        14/10/198301
                          090David Bell          02/07/197902
                          091Emily Reed          20/11/198501
                          092Kenneth Sanders     03/01/197901
                          093Lisa Martinez       11/05/198301
                          094Paul Evans          18/07/199301
                          095Elizabeth Hayes     28/09/198401
                          096Chris Mitchell      23/02/197802
                          097Nancy Stewart       06/06/199302
                          098George Watson       15/12/198301
                          099Charles Bryant      01/03/198501
                          100Jennifer Gray       10/08/197901
                          101Mark Alexander      22/04/198301
                          102Kimberly Rodriguez  27/06/199301
                          103John Ross           30/01/198502
                          104Lisa Diaz           08/09/197901
                          105Richard Henderson   13/03/198301
                          106David Moore         26/05/197802
                          107Jessica Cooper      18/10/199301
                          108John Adams          21/02/198501
                          109Jennifer Mitchell   03/11/197801
                          110Thomas Lewis        28/06/199301
                          111Nancy Scott         12/12/198301
                          112Robert Davis        30/04/197901
                          113Jessica Miller      05/01/198501
                          114Kevin Perez         19/03/197902
                          115Elizabeth Taylor    24/08/198301
                          116John Lee            11/11/199301
                          117Karen Thomas        01/07/198501
                          118Donald Robinson     16/02/197901
                          119Sandra Walker       29/05/198301
                          120Brian Anderson      12/03/197802
                          121Nancy Young         22/09/198301
                          122Sarah Mitchell      17/04/198501
                          123Steven Hall         30/11/197901
                          124Mary Scott          04/05/198301
                          125Charles Allen       14/07/199301
                          126Sandra Garcia       27/02/198501
                          127Joseph Turner       01/10/197901
                          128Emily Young         18/03/198301
                          129Michael Campbell    22/08/199301
                          130Sarah Adams         07/01/198501
                          131Daniel Rodriguez    30/05/197901
                          132Nancy Brown         15/09/198301
                          133Kevin Walker        11/12/199301
                          134Karen White         23/03/198501
                          135James Allen         06/11/197902
                          136Jennifer Lewis      17/06/198301
                          137David Nelson        25/02/197801
                          138Sarah Brown         29/10/199302
                          139Paul Scott          14/04/198501
                          140Nancy Martin        19/01/197901
                          141George Johnson      27/07/198301
                          142Linda Adams         01/09/199302
                          143David Lee           15/05/198501
                          144Emily White         24/02/197902
                          145Steven Anderson     30/08/198301
                          146John Martinez       07/03/199301
                          147Kimberly Robinson   20/12/198501
                          148Brian Young         12/02/197901
                          149Lisa Thomas         05/11/198301
                          150Lucas Rivetti       06/11/198301
                          151Michael Clark       19/06/199301
                          152Jennifer Hall       13/01/198501
                          153Karen King          28/03/197901
                          154George Moore        22/08/198301
                          155Nancy Hill          11/10/199302
                          156Jessica Adams       01/12/198501
                          157Paul Nelson         16/02/197902
                          158Sarah Young         28/07/198301
                          159David Thompson      13/09/199301
                          160Emily Martinez      20/04/198501
                          161Steven Miller       05/06/197902
                          162John Anderson       30/11/198301
                          163Nancy Lewis         17/07/199301
                          """;

        File.WriteAllText("test8.txt", fileContent);

        var reader = new FlatTextFileReader<RecordExample>(new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

        // act
        var result = await reader.ReadFileInParallel("test8.txt");

        // assert
        result.Should().HaveCount(163);

        var orderedList = result.OrderBy(x => x.Number).ToList();

        var g = result.GroupBy(x => x.Number).Where(x => x.Count() > 1).ToList();

        for (int i = 1; i < orderedList.Count; i++)
        {
            if (orderedList[i].Number != orderedList[i - 1].Number + 1)
            {
                throw new InvalidOperationException($"Item at index {i} with number {orderedList[i].Number} is not sequential after item with number {orderedList[i - 1].Number}");
            }
        }

    }
}