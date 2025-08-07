import { Resolver, Query, Args, Mutation } from '@nestjs/graphql';
import { User } from './user';
import { UserService } from './user.service';
import { UseGuards, UsePipes } from '@nestjs/common';
import { CurrentUser } from 'src/auth/current-user.decorator';
import { CurrentUserModel } from 'src/auth/current-user.model';
import { UserUpdateInput } from './inputs/user-update.input';
import { GqlAuthGuard } from 'src/auth/guards/gql-auth.guard';

@UseGuards(GqlAuthGuard)
@Resolver(() => User)
export class UserResolver {
    constructor(private readonly userService: UserService) { }

    // @Query(() => [User])
    // async users() {
    //     return await this.userService.findAll();
    // }

    @Query(() => User)
    async user(@Args('id') id: string) {
        return await this.userService.findById(id);
    }

    @Query(() => User)
    async me(@CurrentUser() user: CurrentUserModel) {
        return await this.userService.findById(user.id);
    }

    @Mutation(() => User)
    async updateUser(@CurrentUser() user: CurrentUserModel, @Args('input') data: UserUpdateInput) {
        return await this.userService.update(user.id, data);
    }
}
