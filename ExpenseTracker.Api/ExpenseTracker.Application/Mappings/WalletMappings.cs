using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mappings;

internal sealed class WalletMappings : Profile
{
    public WalletMappings()
    {
        CreateMap<Wallet, WalletDto>();
        CreateMap<CreateWalletRequest, Wallet>();
        CreateMap<UpdateWalletRequest, Wallet>();
    }
}