using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        // for list of products
        /*
            : base( x => 
            (!brandId.HasValue || x.ProductBrandId == brandId)
            &&
            (!typeId.HasValue || x.ProductTypeId == typeId) )

            this is used for filtering products by type and brand
        */

        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
                                                       : base(x =>
                                                      (string.IsNullOrEmpty(productParams.Search) ||
                                                           x.Name.ToLower().Contains(productParams.Search)) &&
                                                      (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) &&
                                                      (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId))
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);

            // products order by name
            AddOrderBy(x => x.Name);

            ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            // sort products by price ASC and DESC
            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(n => n.Name);
                        break;
                }
            }
        }

        // for one product
        // 'x => x.Id == id' this is replaces with this 'Expression<Func<T, bool>> criteria' in ctor BaseSpecification class
        public ProductsWithTypesAndBrandsSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
        }
    }
}
