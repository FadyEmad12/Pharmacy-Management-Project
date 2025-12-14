using AutoMapper;
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Models.Dto;

namespace Pharmacy.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Drug, DrugsummaryDto>()
             .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.DrugTags.Select(t => t.Tag.Name).ToList()));
            CreateMap<DrugCreateDto, Drug>()
              .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
              .ForMember(dest => dest.DrugTags, opt => opt.Ignore());
            CreateMap<DrugUpdateDto, Drug>()
            .ForMember(dest => dest.DrugTags, opt => opt.Ignore());
            CreateMap<Invoice, InvoiceResponseDto>().ForMember(dest => dest.Items,
        opt => opt.MapFrom(src => src.InvoiceItems)); 

             CreateMap<InvoiceItem, InvoiceItemResponseDto>()
            .ForMember(dest => dest.PricePerUnit,
                opt => opt.MapFrom(src => src.Drug.PurchasingPrice))
            .ForMember(dest => dest.TotalPrice,
                opt => opt.MapFrom(src => src.Drug.PurchasingPrice * src.Quantity));

            CreateMap<Invoice, InvoiceSummaryDto>()
           .ForMember(dest => dest.AdminUsername,
        opt => opt.MapFrom(src => src.Admin != null ? src.Admin.Username : null));
            CreateMap<Invoice, InvoiceDetailsDto>()
          .ForMember(dest => dest.AdminUsername,
        opt => opt.MapFrom(src => src.Admin != null ? src.Admin.Username : null))
          .ForMember(dest => dest.Items,
        opt => opt.MapFrom(src => src.InvoiceItems));
            CreateMap<Invoice, InvoiceDateDto>();

            CreateMap<InvoiceItem, InvoiceItemDetailsDto>()
    .ForMember(dest => dest.DrugName,
        opt => opt.MapFrom(src => src.Drug.Name))
    .ForMember(dest => dest.PricePerUnit,
        opt => opt.MapFrom(src => src.Drug.PurchasingPrice))
    .ForMember(dest => dest.TotalPrice,
        opt => opt.MapFrom(src => src.Drug.PurchasingPrice * src.Quantity));


        }
    }
}
