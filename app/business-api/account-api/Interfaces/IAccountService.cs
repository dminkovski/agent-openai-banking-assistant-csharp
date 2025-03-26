public interface IAccountService
{
    public List<Beneficiary> GetRegisteredBeneficiary(string accountId);
    public PaymentMethod GetPaymentMethodDetails(String paymentMethodId);
    public Account GetAccountDetails(String accountId);    
}
