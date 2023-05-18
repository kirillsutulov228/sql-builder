import { useCallback, useRef } from "react"

export const useDebouncedCallback = (cb, delay, deps) => {
  const ref = useRef();
  
  const debouncedCallback = useCallback(() => {
    clearTimeout(ref.current)
    ref.current = setTimeout(cb, delay);
  }, [...deps, cb, delay])

  return debouncedCallback;
}
