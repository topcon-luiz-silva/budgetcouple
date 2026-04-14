namespace BudgetCouple.Infrastructure.Services.Import;

using BudgetCouple.Application.Import.Interfaces;
using System.Text.RegularExpressions;

/// <summary>
/// Parser for OFX files (both 1.x SGML and 2.x XML formats).
/// </summary>
public class OfxParser : IOfxParser
{
    public async Task<List<OfxTransacao>> ParseAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        var transacoes = new List<OfxTransacao>();

        // Try to parse as OFX 2.x (XML) first
        if (content.TrimStart().StartsWith("<"))
        {
            transacoes = ParseXml(content);
        }
        else
        {
            // Parse as OFX 1.x (SGML)
            transacoes = ParseSgml(content);
        }

        return transacoes;
    }

    private List<OfxTransacao> ParseSgml(string content)
    {
        var transacoes = new List<OfxTransacao>();

        // Extract all STMTTRN blocks
        var stmtTrnPattern = @"<STMTTRN>.*?</STMTTRN>";
        var stmtTrnMatches = Regex.Matches(content, stmtTrnPattern, RegexOptions.Singleline);

        foreach (Match stmtMatch in stmtTrnMatches)
        {
            var stmtBlock = stmtMatch.Value;
            var transacao = ParseStmtBlock(stmtBlock);
            if (transacao != null)
                transacoes.Add(transacao);
        }

        return transacoes;
    }

    private List<OfxTransacao> ParseXml(string content)
    {
        var transacoes = new List<OfxTransacao>();

        // Extract all STMTTRN blocks (XML)
        var stmtTrnPattern = @"<STMTTRN>.*?</STMTTRN>";
        var stmtTrnMatches = Regex.Matches(content, stmtTrnPattern, RegexOptions.Singleline);

        foreach (Match stmtMatch in stmtTrnMatches)
        {
            var stmtBlock = stmtMatch.Value;
            var transacao = ParseStmtBlock(stmtBlock);
            if (transacao != null)
                transacoes.Add(transacao);
        }

        return transacoes;
    }

    private OfxTransacao? ParseStmtBlock(string stmtBlock)
    {
        try
        {
            // Extract TRNTYPE (optional, for validation)
            var trnTypeMatch = Regex.Match(stmtBlock, @"<TRNTYPE>([^<]+)</TRNTYPE>");

            // Extract DTPOSTED (date)
            var dtPostedMatch = Regex.Match(stmtBlock, @"<DTPOSTED>(\d{8,14})</DTPOSTED>");
            if (!dtPostedMatch.Success)
                return null;

            // Extract TRNAMT (amount)
            var trnAmtMatch = Regex.Match(stmtBlock, @"<TRNAMT>([^<]+)</TRNAMT>");
            if (!trnAmtMatch.Success)
                return null;

            // Extract FITID (transaction ID)
            var fitIdMatch = Regex.Match(stmtBlock, @"<FITID>([^<]+)</FITID>");
            if (!fitIdMatch.Success)
                return null;

            // Extract MEMO or NAME (description)
            var memoMatch = Regex.Match(stmtBlock, @"<MEMO>([^<]+)</MEMO>");
            var nameMatch = Regex.Match(stmtBlock, @"<NAME>([^<]+)</NAME>");
            var descricao = memoMatch.Success ? memoMatch.Groups[1].Value
                          : (nameMatch.Success ? nameMatch.Groups[1].Value : "");

            if (string.IsNullOrWhiteSpace(descricao))
                return null;

            // Parse date
            var dateStr = dtPostedMatch.Groups[1].Value;
            var data = ParseOfxDate(dateStr);
            if (data == null)
                return null;

            // Parse amount
            if (!decimal.TryParse(trnAmtMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture, out var valor))
                return null;

            // Convert negative to positive (OFX uses negative for expenses)
            valor = Math.Abs(valor);

            return new OfxTransacao
            {
                Descricao = descricao.Trim(),
                Valor = valor,
                Data = data.Value,
                TransacaoId = fitIdMatch.Groups[1].Value.Trim()
            };
        }
        catch
        {
            return null;
        }
    }

    private DateOnly? ParseOfxDate(string dateStr)
    {
        // OFX date format: YYYYMMDDHHMMSS or YYYYMMDD
        try
        {
            if (string.IsNullOrWhiteSpace(dateStr) || dateStr.Length < 8)
                return null;

            var year = int.Parse(dateStr[..4]);
            var month = int.Parse(dateStr[4..6]);
            var day = int.Parse(dateStr[6..8]);

            return new DateOnly(year, month, day);
        }
        catch
        {
            return null;
        }
    }
}
