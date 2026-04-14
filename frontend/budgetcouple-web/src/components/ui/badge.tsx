import * as React from "react"
import { cn } from "@/lib/utils"

const badgeVariants = {
  default: "bg-slate-100 text-slate-900 border border-slate-300",
  primary: "bg-slate-900 text-white border border-slate-900",
  success: "bg-green-100 text-green-900 border border-green-300",
  warning: "bg-amber-100 text-amber-900 border border-amber-300",
  destructive: "bg-red-100 text-red-900 border border-red-300",
}

export interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement> {
  variant?: keyof typeof badgeVariants
}

function Badge({
  className,
  variant = "default",
  ...props
}: BadgeProps) {
  return (
    <div
      className={cn(
        "inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold",
        badgeVariants[variant],
        className
      )}
      {...props}
    />
  )
}

export { Badge, badgeVariants }
