import { Args, Mutation, Query, Resolver } from '@nestjs/graphql';
import { Organization } from './organization';
import { OrganizationService } from './organization.service';
import { OrganizationCreateInput } from './inputs/organization-create.input';
import { CurrentUser } from 'src/auth/current-user.decorator';
import { CurrentUserModel } from 'src/auth/current-user.model';
import { OrganizationUpdateInput } from './inputs/organization-update.input';

@Resolver(() => Organization)
export class OrganizationResolver {
    constructor(private readonly organizationService: OrganizationService) { }

    @Query(() => [Organization])
    async oranizations() {
        return await this.organizationService.findAll();
    }

    @Query(() => Organization)
    async oranization(@Args('id') id: string) {
        return await this.organizationService.findById(id);
    }

    @Mutation(() => Organization)
    async createOrganization(@CurrentUser() user: CurrentUserModel, @Args('data') data: OrganizationCreateInput) {
        return await this.organizationService.create(data, user.id);
    }

    @Mutation(() => Organization)
    async updateOrganization(@CurrentUser() user: CurrentUserModel, @Args('id') id: string, @Args('data') data: OrganizationUpdateInput) {
        return await this.organizationService.update(id, user.id, data);
    }
}
