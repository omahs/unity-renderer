﻿using DCL.Interface;

public struct ChatEntryModel
{
    public enum SubType
    {
        NONE,
        RECEIVED,
        SENT
    }

    public ChatMessage.Type messageType;
    public string bodyText;
    public string senderId;
    public string senderName;
    public string recipientName;
    public string otherUserId;
    public ulong timestamp;

    public SubType subType;
}