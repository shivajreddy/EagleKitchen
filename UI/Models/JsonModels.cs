using System.Collections.Generic;

namespace EK24_old.JsonModels;

// Models for ek24_brands_styles.json
public class JsonBrandsStylesModel
{
    public List<BrandStylesModel> Brands { get; set; }
}
public class BrandStylesModel
{
    public string BrandName { get; set; }
    public List<StyleModel> Styles { get; set; }
}
public class StyleModel
{
    public string StyleName { get; set; }
    public List<string> Properties { get; set; }
    public List<SpeciesVariantModel> SpeciesVariants { get; set; }
}

public class SpeciesVariantModel
{
    public string Species { get; set; }
    public List<string> Finishes { get; set; }
}


// Models for ek24_brands_shapes_types.json
public class JsonBrandsShapesTypesModel
{
    public List<BrandShapesTypesModel> Brands { get; set; }
}
public class BrandShapesTypesModel
{
    public string BrandName { get; set; }
    public List<ShapeTypesModel> Shapes { get; set; }
}
public class ShapeTypesModel
{
    public string ShapeName { get; set; }
    public List<string> Types { get; set; }
}
