namespace Auth.Api.Features.Addresses.Delete;

public sealed class DeleteAddressHandler(IUserAddressRepository addressRepository)
    : IRequestHandler<DeleteAddressCommand, Result>
{
    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var deleted = await addressRepository.DeleteAsync(request.UserId, request.AddressId, cancellationToken);
        return deleted
            ? Result.Success()
            : Result.NotFound("address.not_found", "Address was not found.");
    }
}
