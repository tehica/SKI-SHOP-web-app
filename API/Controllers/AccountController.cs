using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ITokenService tokenService,
                                 IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        // use this method to get the currently logged in user
        public async Task<ActionResult<UserDto>> GetCurrentlyUser()
        {
            // FindByEmailFromClaimsPrinciple method is stored in Extensions/UserManagerExtensions class
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        // check if and email address is already in use
        [HttpGet("emailexists")]
        // [FromQuery] string email get the string of the email from query thats give the API a hint
        // gives us clarity about where the email address is coming from
        // this is probably unnecessary ( [FromQuery] )
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            // if this is true thats mean user with this email exist
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        // return users address
        [HttpGet("address")]
        // in ActionResult we define what we returning in this case is: <AddressDto>
        public async Task<ActionResult<AddressDto>> GetUserAdress()
        {
            // FindByEmailFromClaimsPrinciple method is stored in Extensions/UserManagerExtensions class
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(HttpContext.User);
            return _mapper.Map<Address, AddressDto>(user.Address);
        }


        [Authorize]
        // HttpPut attribute is used for update
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            // get user address
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            user.Address = _mapper.Map<AddressDto, Address>(address);
            // update user with userManager
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok( _mapper.Map<Address, AddressDto>(user.Address) );
            }

            return BadRequest("Problem with updating the user");
        }


        // in this POST method we use UserDto class that we should not bring back whole AppUser object
        // because this whole AppUser object contains the majority properties which we don't use
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // userManager is used to get user from db
            // signInManager is used to check the user's password what is stored in db
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }
            // CheckPasswordSignInAsync method which is going to attempt to password sign in for the user
            // this takes the user to sign in the User objects which we've got from the user manager
            // it takes the password which is inside our loginDto 
            // and it takes a flag (false) to indicate is user locked out and if it is sign in fails
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            // if login is not succeeded ...
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if email is already exist and then we are going to send a response back to tell
            // user that is already exists
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse{
                    Errors = new[] {
                    "Email address is in use"
                }});
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest( new ApiResponse(400));
            }

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email
            };
        }
    }
}
