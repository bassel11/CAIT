﻿namespace Identity.Application.DTOs.Roles
{
    public class RoleFilterDto
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
