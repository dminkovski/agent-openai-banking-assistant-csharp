﻿using agent_openai_banking_assistant_csharp.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

public class InvoiceScanPlugin
{
    private readonly ILogger<InvoiceScanPlugin> _logger;
    private IDocumentScanner _documentScanner;
    public InvoiceScanPlugin(IDocumentScanner documentScanner, ILogger<InvoiceScanPlugin> _logger)
    {
        _documentScanner = documentScanner;
        _logger = _logger;
    }
    [KernelFunction("scanInvoice")]
    [Description("Extract the invoice or bill data scanning a photo or image")]
    public string ScanInvoice([DescriptionAttribute("the path to the file containing the image or photo")] string filePath) {

        Dictionary<string, string> scanData = null;
        _logger.LogInformation($"Attempting to scan: {filePath}");

        try{
            scanData = _documentScanner.Scan(filePath);
        } catch (Exception e) {
          _logger.LogError($"Error extracting data from invoice {filePath}: {e.ToString()}");
            scanData = new();
        }

        _logger.LogInformation($"SK scanInvoice plugin: Data extracted {filePath}:{scanData}");
        return scanData.ToString();
    }
}

