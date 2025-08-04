# 🦄 sso-api-v2
## 🚀login
#### Input Body
##### application/json
```json
{
      "username":"0064344",
      "password":"Cgs#1234"
}
```

#### Output

##### Status 200
```json
{
      "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MjY5MTMyMzIsImlzX3JlZnJlc2hfdG9rZW4iOmZhbHNlLCJ1c2VyX2lkIjoiZGJlNDY2N2MtZWViMy00OWUzLWEwMmUtM2I4ZDAzYzU5MjJjIn0.Q517ahtUUiwNUvEQxcA8_BYdX-tmIo9vZzvq4Xjc2x8",
      "access_token_expiry": "24h",
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3Mjc0MzE2MzIsImlzX3JlZnJlc2hfdG9rZW4iOnRydWUsInVzZXJfaWQiOiJkYmU0NjY3Yy1lZWIzLTQ5ZTMtYTAyZS0zYjhkMDNjNTkyMmMifQ.-HL8Op1SrC6WhsbLKXRFJkEHD6Xt5v_HKBuc6iJdEyY",
      "refresh_token_expiry": "168h"
  }
```
##### Status 401
```json
  {
      "error": "{{error_message}}"
  }
```
##### process
    check if username and password is correct
    if correct, generate access token and refresh token , return access token and refresh token with status 200
    if incorrect, return error message with status 401
    If the login fails more than 5 times, the account will be locked until it is unlocked by an admin.

###### ⛔️ note
Customer can have multiple sets of username and password. Each set works separately when logging in. The data refers to the same user as follows:

    email paired with password
    custcode paired with password

###### more
The user's password will be encrypted in 2 ways, separated into the old way which is sha-1 without salt_password and the new way which is bcrypt with salt_password

When the password is reset, it will be automatically changed to the new encryption method

## 🚀refresh-token
#### input header
`Authorization: Bearer <refresh_token>`

#### Output

##### Status 200
```json
  {
      "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MjY5MTMyMzIsImlzX3JlZnJlc2hfdG9rZW4iOmZhbHNlLCJ1c2VyX2lkIjoiZGJlNDY2N2MtZWViMy00OWUzLWEwMmUtM2I4ZDAzYzU5MjJjIn0.Q517ahtUUiwNUvEQxcA8_BYdX-tmIo9vZzvq4Xjc2x8",
      "access_token_expiry": "24h",
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3Mjc0MzE2MzIsImlzX3JlZnJlc2hfdG9rZW4iOnRydWUsInVzZXJfaWQiOiJkYmU0NjY3Yy1lZWIzLTQ5ZTMtYTAyZS0zYjhkMDNjNTkyMmMifQ.-HL8Op1SrC6WhsbLKXRFJkEHD6Xt5v_HKBuc6iJdEyY",
      "refresh_token_expiry": "168h"
  }
```
##### Status 401
```json
  {
      "error": "{{error_message}}"
  }
```
###### process
    check if refresh token is valid
    if valid, generate new access token and refresh token, return access token and refresh token with status 200
    if invalid, return error message with status 401

## 🚀 create-pin
#### input header
`Authorization: Bearer <refresh_token>`
#### input body
##### application/json
```json
  {
      "custcode":["0064344"],
      "new_pin":"123456"
  }
```
#### Output

##### Status 200
```json
  {
      "message": "Pin created successfully"
  }
```
##### Status 401
```json
  {
      "error": "{{error_message}}"
  }
```

    Good, create new pin and sync to Settrade, return message with status 200
    Not, return error message with status 401

## 🚀change-pin
#### input header
`Authorization: Bearer <refresh_token>`
#### input body
##### application/json
```json
{
      "custcode":["0064344"],
      "old_pin":"123456",
      "new_pin":"654321"
}
```
#### Output

##### Status 200
```json
{
    "message": "Pin changed successfully"
}
```
##### Status 401
```json
{
    "error": "{{error_message}}"
}
```
###### process
    check if old pin is correct
    if correct, change pin and sync to Settrade, return message with status 200
    if incorrect, return error message with status 401
    📌 We need to have a pin lock?

## 🚀 verify-pin
#### input header
`Authorization: Bearer <refresh_token>`
#### input body
##### application/json
```json
{
    "custcode":"0064344",
    "pin":"123456"
}
```
##### Status 200
```json
{
    "message": "Pin verified successfully"
}
```
##### Status 401
```json
{
    "error": "{{error_message}}"
}
```
##### process
    check if pin is correct
    if correct, return message with status 200
    if incorrect, return error message with status 401
###### more
    The user's pin will be encrypted in 2 ways, separated into the old way which is sha-1 without salt_pin and the new way which is bcrypt with salt_pin
    When the pin is reset, it will be automatically changed to the new encryption method
      📌 We need to have a pin lock?

## 🚀 change-password
#### input header
`Authorization: Bearer <refresh_token>`
#### input body
##### application/json
```json
{
    "username":"0064344",
    "old_password":"Cgs#1234",
    "new_password":"Cgs#4321"
}
```
##### Status 200
```json
{
    "message": "Password changed successfully"
}
```
##### Status 401
```json
{
    "error": "{{error_message}}"
}
```
##### process
    check if old password is correct
    if correct, change password and sync to Settrade and return message with status 200
    if incorrect, return error message with status 401

## 🚀 guest-register
#### input body
##### application/json
```json
{
    "username":"guest_register@email.address",
    "password":"abc#1234"
}

```
##### Status 200
```json
{
    "message": "Guest register successfully"
}
```
##### Status 401
```json
{
    "error": "{{error_message}}"
}
```
##### process
    check if username is not exist
    if not exist, create new guest account and return message with status 200
    if exist, return error message with status 401

## 🚀account/link
#### input header
`Authorization: Bearer <refresh_token>`
#### input body
##### application/json
```json
{
    "custcode":"0064344"
}
```
##### Status 200
```json
{
    "message": "Account linked successfully"
}
```
##### Status 401
```json
{
    "error": "{{error_message}}"
}
```
##### process
    check if cardId  between authenticated and custcode is matched
    if matched, link account and return message with status 200
    if not matched, return error message with status 401
