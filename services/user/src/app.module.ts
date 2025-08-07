
import { Module } from '@nestjs/common';
import { GraphQLModule } from '@nestjs/graphql';
import { ApolloDriver, ApolloDriverConfig } from '@nestjs/apollo';
import { AuthModule } from './auth/auth.module';
import { PrismaModule } from './prisma/prisma.module';
import { UserModule } from './user/user.module';
import { ClassroomModule } from './classroom/classroom.module';
import { OrganizationModule } from './organization/organization.module';
import { ClassroomUserModule } from './classroom-user/classroom-user.module';
import { ClassroomCourseModule } from './classroom-course/classroom-course.module';
import { join } from 'path';
import { GraphQLError } from 'graphql';

@Module({
  imports: [
    GraphQLModule.forRoot<ApolloDriverConfig>({
      driver: ApolloDriver,
      autoSchemaFile: true,
      // autoSchemaFile: join(process.cwd(), 'src/schema.gql'),
      graphiql: true,
      formatError: (error: GraphQLError) => {
        return process.env.NODE_ENV !== 'production' ? error :
          {
            message: error.message,
            locations: error.locations,
            path: error.path,
          };
      },
    }),
    AuthModule,
    PrismaModule,
    UserModule,
    OrganizationModule,
    ClassroomModule,
    ClassroomUserModule,
    ClassroomCourseModule,
  ],
})
export class AppModule { }
