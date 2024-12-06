using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Requests.Transfer;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mappings;

internal sealed class TransferMappings : Profile
{
    public TransferMappings()
    {
        CreateMap<Transfer, TransferDto>();
        CreateMap<CreateTransferRequest, Transfer>();
        CreateMap<UpdateTransferRequest, Transfer>();
    }
}