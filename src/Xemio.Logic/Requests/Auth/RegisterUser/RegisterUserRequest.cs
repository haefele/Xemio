﻿using FluentValidation;
using MediatR;
using Xemio.Logic.Entities;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Auth.RegisterUser
{
    public class RegisterUserRequest : IRequest<User>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            this.RuleFor(f => f.EmailAddress).NotEmpty().EmailAddress().NoSurroundingWhitespace();
            this.RuleFor(f => f.Password).NotEmpty().MinimumLength(8);
        }
    }
}