import {Injectable} from "@angular/core";
import {Apollo, gql} from "apollo-angular";
import {map} from "rxjs";
import { User } from "src/models";

export interface Post {
  id: number;
  title: string;
  content: string;
  author?: User
}

interface PostResponse {
  posts: Post[];
}

const GET_POSTS = gql`
  query GetPosts {
    posts {
      id
      title
      content
      author {
        id
        fullName
      }
    }
  }
`;

@Injectable()
export class PostsFeedService {
  constructor(private readonly apollo: Apollo) {
  }

  getPosts() {
    return this.apollo.query<PostResponse>({query: GET_POSTS})
      .pipe(map(x => x.data.posts));
  }
}
