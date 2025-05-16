﻿namespace ToDoList.Dal.Entity;

public class RefreshToken
{
    public long RefreshTokenId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public bool IsRevoked { get; set; }

    public long UserId { get; set; }
    public User User { get; set; }
}
