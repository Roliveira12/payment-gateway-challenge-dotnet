﻿using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public string Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}
