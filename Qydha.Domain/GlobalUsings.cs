﻿global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.JsonPatch;

global using System.Security.Claims;
global using System.Text;

global using Qydha.Domain.Exceptions;
global using Qydha.Domain.Extensions;
global using Qydha.Domain.Entities;
global using Qydha.Domain.Repositories;
global using Qydha.Domain.Enums;
global using Qydha.Domain.Common;
global using Qydha.Domain.Settings;
global using Qydha.Domain.Services.Contracts;
global using Qydha.Domain.MediatorNotifications;
global using Qydha.Domain.Constants;
global using Qydha.Domain.Services.Implementation;
global using Qydha.Domain.ValueObjects;
global using Qydha.Domain.Models;
global using Qydha.Domain.Hubs;
global using Microsoft.Extensions.Logging;

global using Riok.Mapperly.Abstractions;

global using MediatR;
global using Newtonsoft.Json;
global using FluentResults;
