using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ReStyle.Helpers;

public class ItemsChangedMessage : ValueChangedMessage<bool>
{
    public ItemsChangedMessage() : base(true) { }
}
