﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ShopHub.Core.Entities;
using ShopHub.Core.Interfaces;
using ShopHub.Core.Services;
using ShopHub.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;
        private readonly IConnectionMultiplexer redis;
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IGenerateToken token;
        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IPhotoRepository PhotoRepository {  get; }

        public ICustomerBasketRepository CustomerBasket {  get; }

        public IAuth Auth { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper,
            IImageManagementService imageManagementService,
            IConnectionMultiplexer redis, UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken token)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
            this.redis = redis;
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.token = token;
            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context, _mapper, _imageManagementService);
            PhotoRepository = new PhotoRepository(_context);
            CustomerBasket = new CustomerBasketRepository(redis);
            Auth = new AuthRepository(userManager, emailService, signInManager, token);
        }
    }
}
