// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Pi.GlobalEquities.Worker.ExternalServices.Notification;

public class NotificationApiConfig
{
    public const string Name = "NotificationApi";
    public string Url { get; init; }
    public TimeSpan Timeout { get; init; }
    public string Mode { get; init; }
}
