
// Record classes for data models
public record Payment(
    string AccountId,
    string Description,
    string PaymentType,
    string PaymentMethodId,
    string RecipientName,
    string RecipientBankCode,
    decimal Amount,
    DateTime Timestamp
);

public record Transaction(
    string Id,
    string Description,
    string Type,
    string RecipientName,
    string RecipientBankCode,
    string AccountId,
    string PaymentType,
    decimal Amount,
    DateTime Timestamp
);