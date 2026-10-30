using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetForge.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeGuardFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DashboardLayouts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "TenantInvitations");

            migrationBuilder.DropTable(
                name: "WebhookDeliveries");

            migrationBuilder.DropTable(
                name: "WebhookSubscriptions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ReportedDate = table.Column<long>(type: "INTEGER", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    YearBuilt = table.Column<int>(type: "INTEGER", nullable: true),
                    Rooms = table.Column<int>(type: "INTEGER", nullable: true),
                    SquareFeet = table.Column<long>(type: "INTEGER", precision: 12, scale: 2, nullable: true),
                    HealthScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ObservedValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RecommendedAction = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MetricType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SensorId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    MetricType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<long>(type: "INTEGER", precision: 12, scale: 4, nullable: false),
                    RecordedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_PropertyId_Status",
                table: "Incidents",
                columns: new[] { "PropertyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_City",
                table: "Properties",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Name",
                table: "Properties",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RiskEvents_PropertyId_CreatedAt",
                table: "RiskEvents",
                columns: new[] { "PropertyId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_PropertyId",
                table: "Sensors",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_PropertyId_RecordedAt",
                table: "TelemetryReadings",
                columns: new[] { "PropertyId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_SensorId",
                table: "TelemetryReadings",
                column: "SensorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "RiskEvents");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "TelemetryReadings");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Changes = table.Column<string>(type: "TEXT", nullable: true),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    TraceId = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardLayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    LayoutJson = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    ReadAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AcceptedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<long>(type: "INTEGER", nullable: false),
                    InvitedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: true),
                    Error = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NextAttemptAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseBody = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDeliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Events = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Secret = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    DeletedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Images = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false),
                    RowVersion = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    StockQty = table.Column<int>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    PlacedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Subtotal = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false),
                    Tax = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Total = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    LineTotal = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Qty = table.Column<int>(type: "INTEGER", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    UnitPrice = table.Column<long>(type: "INTEGER", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLines_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_Timestamp",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardLayouts_TenantId_UserId_Name",
                table: "DashboardLayouts",
                columns: new[] { "TenantId", "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_ReadAt",
                table: "Notifications",
                columns: new[] { "UserId", "ReadAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductId",
                table: "OrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_TenantId",
                table: "OrderLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TenantId",
                table: "Orders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvitations_Email",
                table: "TenantInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvitations_TenantId",
                table: "TenantInvitations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvitations_TokenHash",
                table: "TenantInvitations",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_SubscriptionId_CreatedAt",
                table: "WebhookDeliveries",
                columns: new[] { "SubscriptionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WebhookSubscriptions_TenantId",
                table: "WebhookSubscriptions",
                column: "TenantId");
        }
    }
}
