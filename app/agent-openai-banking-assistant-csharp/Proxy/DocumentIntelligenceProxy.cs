
using Microsoft.Extensions.Logging;

public class DocumentIntelligenceProxy : IDocumentScanner
{
    private readonly IBlobStorage _blobStorageProxy;

    private readonly DocumentIntelligenceClient _documentIntelligenceClient;

    private ILogger _logger;


    public DocumentIntelligenceProxy(IBlobStorage blobStorageProxy, DocumentIntelligenceClient documentIntelligenceClient, ILoggerFactory loggerFactory)
    {
        _blobStorageProxy = blobStorageProxy;
        _documentIntelligenceClient = documentIntelligenceClient;
        _logger = loggerFactory.CreateLogger<DocumentIntelligenceProxy>();
    }

    public Dictionary<string, string> Scan(string fileName)
    {
        Uri uriSource = _blobStorageProxy.GetFileUri(fileName);
        _logger.LogInformation($"Scanning: {uriSource.ToString()}");
        string modelId = "prebuilt-invoice";
        Operation<AnalyzeResult> operation = _documentIntelligenceClient.AnalyzeDocument(WaitUntil.Completed, modelId, uriSource);
        AnalyzeResult result = operation.Value;

        Dictionary<string, string> scanData = new Dictionary<string, string>();

        for (int i = 0; i < result.Documents.Count; i++)
        {
            AnalyzedDocument analyzedInvoice = result.Documents[i];

            if (analyzedInvoice.Fields.TryGetValue("VendorName", out DocumentField vendorNameField)
                && vendorNameField.FieldType == DocumentFieldType.String)
            {
                scanData.Add("VendorName", vendorNameField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("VendorAddress", out DocumentField vendorAddressField)
                && vendorAddressField.FieldType == DocumentFieldType.Address)
            {
                scanData.Add("VendorAddress", vendorAddressField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("CustomerName", out DocumentField customerNameField)
                && customerNameField.FieldType == DocumentFieldType.String)
            {
                scanData.Add("CustomerName", customerNameField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("CustomerAddressRecipient", out DocumentField customerAddressRecipientField)
                && customerAddressRecipientField.FieldType == DocumentFieldType.String)
            {
                scanData.Add("CustomerAddressRecipient", customerAddressRecipientField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("InvoiceId", out DocumentField invoiceIdField)
                && invoiceIdField.FieldType == DocumentFieldType.String)
            {
                scanData.Add("InvoiceId", invoiceIdField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("InvoiceDate", out DocumentField invoiceDateField)
                && invoiceDateField.FieldType == DocumentFieldType.Date)
            {
                scanData.Add("InvoiceDate", invoiceDateField.ValueString);
            }
            if (analyzedInvoice.Fields.TryGetValue("InvoiceTotal", out DocumentField invoiceTotalField)
                && invoiceTotalField.FieldType == DocumentFieldType.String)
            {
                scanData.Add("InvoiceTotal", invoiceTotalField.ValueString);
            }

        }
        return scanData;
    }
}
