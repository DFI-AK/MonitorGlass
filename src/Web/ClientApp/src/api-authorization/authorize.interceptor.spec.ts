import { TestBed, inject } from '@angular/core/testing';

import { authorizeInterceptor } from './authorize.interceptor';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [authorizeInterceptor],
    });
  });

  it('should be created', inject(
    [authorizeInterceptor],
    (service: typeof authorizeInterceptor) => {
      expect(service).toBeTruthy();
    }
  ));
});
