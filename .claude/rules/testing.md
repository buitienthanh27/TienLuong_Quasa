# Testing — API_Sample

> Hiện solution chưa có project test. Khi thêm, tuân thủ rule sau.

## Setup

- Project test đặt cùng cấp solution: `API_Sample.Tests.Unit`, `API_Sample.Tests.Integration`.
- Framework: **xUnit** + **FluentAssertions** + **Moq** (hoặc NSubstitute).
- Integration test dùng `Microsoft.AspNetCore.Mvc.Testing` + `Microsoft.EntityFrameworkCore.InMemory` hoặc SQL Server LocalDB riêng.

## Naming

- File: `{ClassUnderTest}Tests.cs` (vd `S_ProductTests.cs`).
- Method: `{Method}_{Scenario}_{ExpectedResult}` (vd `Create_DuplicateCode_ReturnsConflict`).

## Cấu trúc test (AAA)

```csharp
[Fact]
public async Task Create_DuplicateCode_ReturnsConflict()
{
    // Arrange
    var ctx = BuildInMemoryContext();
    ctx.Products.Add(new Product { Code = "P01", Status = 1 });
    await ctx.SaveChangesAsync();
    var sut = new S_Product(ctx, mapper, NullLogger<S_Product>.Instance);

    // Act
    var result = await sut.Create(new MReq_Product { Code = "P01", Name = "X" });

    // Assert
    result.result.Should().Be(0);
    result.error.code.Should().Be((int)HttpStatusCode.Conflict);
}
```

## Coverage target

- Service (`S_X`): mọi method có ít nhất 1 happy path + 1 error path (`Error()` branch) + 1 exception path (`CatchException` branch).
- Helper trong `Utilities/`: 100% nhánh.
- Controller: smoke test qua integration test (1 happy path mỗi action).

## Mock / fake

- DbContext: dùng `UseInMemoryDatabase` hoặc SQLite in-memory cho test EF.
- AutoMapper: dùng `MapperConfiguration` thật với `AutoMapperProfile` — không mock IMapper.
- ILogger: `NullLogger<T>.Instance`.

## Chạy test

```bash
dotnet test API_Sample.sln
```

## Cấm

- Không assert trên chuỗi message tiếng Việt cụ thể (dễ vỡ khi sửa text) — dùng constant trong `MessageErrorConstants`.
- Không gọi DB production trong test.
- Không skip test bằng `[Fact(Skip="...")]` mà không có ticket tracking.
