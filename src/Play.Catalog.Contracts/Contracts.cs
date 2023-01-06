namespace Play.Catalog.Contracts
{
    // only list stuff the consumer is interested in
    public record CatalogItemCreated(Guid ItemId, string Name, String Description);

    public record CatalogItemUpdated(Guid ItemId, string Name, String Description);

    public record CatalogItemDeleted(Guid ItemId);
}