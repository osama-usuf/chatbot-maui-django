﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiUI.Data;

public class RefreshMessage : ValueChangedMessage<bool>
{
    public RefreshMessage(bool value) : base(value)
    {
    }
}