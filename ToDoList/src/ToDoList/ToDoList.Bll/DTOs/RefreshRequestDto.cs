﻿namespace ToDoList.Bll.DTOs;

public class RefreshRequestDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
