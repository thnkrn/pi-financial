#!/bin/sh
topic='arn:aws:sns:ap-southeast-1:754259340832:kkp-payment-callback.fifo';
customerCode='7711558';
awsProfile='pi-dev-nonprod';
message='
{
  "isSuccess": true,
  "amount": 3240,
  "customerCode": "7711558",
  "product": "mt5",
  "transactionNo": "MT5QR20231003221520",
  "transactionRefCode": "MT520231003000001",
  "paymentDateTime": "20230622143623",
  "payerName": "นางสาว มะขาม หวาน",
  "payerBankCode": "069",
  "payerAccountNo": "2001774052"
}
';

aws sns publish --topic-arn "$topic" --message "$message" --message-group-id "$customerCode" --profile "$awsProfile"
