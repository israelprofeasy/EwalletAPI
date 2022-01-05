using AutoMapper;
using Ewallet.Dtos.Transaction;
using Ewallet.Dtos.Currency;
using Ewallet.Dtos.Role;
using Ewallet.Dtos.User;
using Ewallet.Dtos.Wallet;
using Ewallet.Dtos.WalletCurrency;
using Ewallet.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Ewallet.Dtos.Fund;

namespace Ewallet
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, GetUserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            
            CreateMap<AddWalletDto, Wallet>();
            CreateMap<Wallet, GetWalletDto>()
                .ForMember(dto => dto.WalletCurrencies, w => w.MapFrom(w => w.WalletCurrencies.Select(wc => new GetWalletCurrencyDto
                {
                    CurrencyId = wc.CurrencyId,
                    Name = wc.Currency.Name,
                    ShortCode = wc.Currency.ShortCode,
                    IsMain = wc.IsMain,
                    Balance = wc.Balance
                })));

            CreateMap<AddCurrencyDto, Currency>();
            CreateMap<Currency, GetCurrencyDto>();

            CreateMap<AddRoleDto, IdentityRole>();
            CreateMap<IdentityRole, GetRoleDto>();


            CreateMap<AddWalletCurrencyDto, WalletCurrency>();
            CreateMap<WalletCurrency, GetWalletCurrencyDto>();

            CreateMap<AddTransactionDto, Transaction>();
            CreateMap<FundWalletDto, Transaction>();
            CreateMap<WithdrawDto, Transaction>();
            CreateMap<Transaction, GetTransactionDto>();
        }
    }
}
