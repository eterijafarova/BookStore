namespace BookShop.Data.Enums;

public enum ChatStatus
{
    Waiting = 1,     // ожидает администратора

    InProgress = 2,  // чат взят админом

    Closed = 3       // чат закрыт
}