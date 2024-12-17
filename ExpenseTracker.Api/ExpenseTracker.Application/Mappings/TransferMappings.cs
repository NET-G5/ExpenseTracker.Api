using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Requests.Transfer;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mappings;

internal sealed class TransferMappings : Profile
{
    public TransferMappings()
    {
        CreateMap<Transfer, TransferDto>()
            .ForMember(x => x.CategoryName, cfg => cfg.MapFrom(e => e.Category.Name))
            .ForMember(x => x.WalletName, cfg => cfg.MapFrom(e => e.Wallet.Name));
        CreateMap<CreateTransferRequest, Transfer>()
            .ForMember(x => x.Date, cfg => cfg.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateTransferRequest, Transfer>();
    }
}