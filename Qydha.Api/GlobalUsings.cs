global using Qydha.Domain.Services.Contracts;
global using Qydha.Domain.Services.Implementation;
global using Qydha.Domain.Entities;
global using Qydha.Domain.Common;
global using Qydha.Domain.Enums;

global using Qydha.Domain.Repositories;
global using Qydha.Domain.Settings;
global using Qydha.Domain.Constants;
global using Qydha.Domain.Exceptions;

global using Qydha.Infrastructure;
global using Qydha.Infrastructure.Services;
global using Qydha.Infrastructure.Repositories;
global using Qydha.Infrastructure.Settings;
global using Qydha.API.Models;
global using Qydha.API.Settings;
global using Qydha.API.Mappers;
global using Qydha.API.Validators;
global using Qydha.API.Attributes;
global using Qydha.API.Extensions;
global using Qydha.API.Binders;
global using Qydha.API.Middlewares;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.JsonPatch;

global using Riok.Mapperly.Abstractions;

global using System.Text.RegularExpressions;

global using System;
global using System.Data;
global using System.Text;

global using Serilog;
global using Serilog.Events;
global using Serilog.Formatting.Json;

global using FirebaseAdmin;
global using Google.Apis.Auth.OAuth2;
global using Microsoft.IdentityModel.Tokens;
global using FluentResults;

global using FluentValidation;
global using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

global using Microsoft.AspNetCore.JsonPatch.Exceptions;